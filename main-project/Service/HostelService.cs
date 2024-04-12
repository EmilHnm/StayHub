using main_project.Models;
using main_project.Models.ViewModels;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;

namespace main_project.Service;

public class HostelService
{
    private static HostelService? _instance = null;
    private HostelService()
    {
    }

    public static HostelService Instance()
    {
        return _instance ??= new HostelService();
    }

    public ICollection<Hostel> GetHostels()
    {
        return Hostel.Query().Include(h => h.HostelImages).ToList();
    }

    public ICollection<Hostel> Search(string? query, int type)
    {
        IQueryable<Hostel> DataQuery = Hostel.Query().Include(h => h.HostelImages);
        if ((query == null || query == "") && type == 0)
        {
            return new List<Hostel>();
        }
        if (query != null)
        {
            DataQuery = DataQuery.Where(h => (h.Name ?? "").ToLower().Contains(query.ToLower()) ||
                    (h.Description ?? "").ToLower().Contains(query.ToLower()) ||
                    (h.Address ?? "").ToLower().Contains(query.ToLower()));
        }
        if (type != 0)
        {
            DataQuery = DataQuery.Where(h => h.AccommodationTypeId == type);
        }
        return DataQuery.ToList();
    }

    public ICollection<Hostel> LatestHostels()
    {
        return Hostel.Query().Include(h => h.HostelImages).OrderByDescending(h => h.CreatedAt).Take(6).ToList();
    }

    public Hostel? GetHostel(int id)
    {
        return Hostel.Query().Where(h => h.Id == id)
            .Include(h => h.HostelImages)
            .Include(h => h.HostProfile).ThenInclude(h => h.User)
            .Include(h => h.AccommodationType)
            .Include(h => h.HostelUtility).ThenInclude(h => h.Utility)
            .FirstOrDefault();
    }

    public Hostel? CreateHostel(Hostel hostel, List<string> ImagePath, List<int>? UtilitiesId)
    {
        try
        {
            hostel.CreatedAt = DateTime.UtcNow;
            hostel.UpdatedAt = DateTime.UtcNow;
            DatabaseContext db = new DatabaseContext();
            db.hostels.Add(hostel);
            db.SaveChanges();
            foreach (string path in ImagePath)
            {
                HostelImages image = new HostelImages()
                {
                    HostelId = hostel.Id,
                    ImageUrl = path
                };
                db.Add(image);
            }
            if (UtilitiesId != null)
            {
                foreach (int utilityId in UtilitiesId)
                {
                    HostelUtility utility = new HostelUtility()
                    {
                        HostelId = hostel.Id,
                        UtilityId = utilityId
                    };
                    db.Add(utility);
                }
            }
            db.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
        return hostel;
    }

    public Hostel? UpdateHostel(VM_HostelForm vm)
    {
        Hostel? hostel = Hostel.Query().Where(h => h.Id == vm.Id).FirstOrDefault();
        if (hostel == null)
        {
            return null;
        }
        DatabaseContext db = new DatabaseContext();
        hostel.Name = vm.Name;
        hostel.Description = vm.Description;
        hostel.Address = vm.Address;
        hostel.Fee = vm.Fee;
        hostel.Rooms = vm.Rooms;
        hostel.AccommodationTypeId = vm.AccommodationTypeId;
        if (vm.Status != null) hostel.Status = vm.Status;
        hostel.UpdatedAt = DateTime.UtcNow;
        if (vm.ImageFileURLs != null)
        {
            List<HostelImages> shouldDeleteImages = HostelImages.Query()
            .Where(i => i.HostelId == vm.Id)
            .Where(i => !vm.ImageFileURLs.Contains(i.ImageUrl ?? ""))
            .ToList();
            foreach (HostelImages image in shouldDeleteImages)
            {
                DeleteHostelImage(image.Id);
            }
        }

        if (vm.Utilities != null)
        {
            List<HostelUtility> shouldDeleteUtilities = HostelUtility.Query()
                .Where(u => u.HostelId == vm.Id)
                .Where(u => !vm.Utilities.Contains(u.UtilityId))
                .ToList();
            foreach (HostelUtility utility in shouldDeleteUtilities)
            {
                db.hostelUtilities.Remove(utility);
            }
            List<int> currentUtilities = HostelUtility.Query()
                .Where(u => u.HostelId == vm.Id)
                .Select(u => u.UtilityId)
                .ToList();
            foreach (int utilityId in vm.Utilities)
            {
                if (!currentUtilities.Contains(utilityId))
                {
                    HostelUtility utility = new HostelUtility()
                    {
                        HostelId = hostel.Id,
                        UtilityId = utilityId
                    };
                    db.hostelUtilities.Add(utility);
                }
            }
        }

        List<string> imagePath = SaveImages(vm.ImageFiles);
        if (imagePath.Count > 0)
        {
            foreach (string path in imagePath)
            {
                HostelImages image = new HostelImages()
                {
                    HostelId = hostel.Id,
                    ImageUrl = path
                };
                db.hostelImages.Add(image);
            }
        }

        db.Update(hostel);
        db.SaveChanges();

        return hostel;
    }

    public bool DeleteHostel(int id)
    {
        try
        {
            DatabaseContext db = new DatabaseContext();
            Hostel? hostel = db.hostels.Where(h => h.Id == id).FirstOrDefault();
            if (hostel == null)
            {
                return false;
            }
            List<HostelImages> images = HostelImages.Query().Where(i => i.HostelId == id).ToList();
            foreach (HostelImages image in images)
            {
                DeleteHostelImage(image.Id);
            }
            List<HostelUtility> utilities = HostelUtility.Query().Where(u => u.HostelId == id).ToList();
            foreach (HostelUtility utility in utilities)
            {
                db.hostelUtilities.Remove(utility);
            }
            db.hostels.Remove(hostel);
            db.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public List<Hostel> GetHostelsByHostId(int hostId)
    {
        return Hostel.Query().Where(h => h.HostId == hostId).ToList();
    }

    public bool SaveHostelImage(int hostelId, string imageFile)
    {
        try
        {
            HostelImages image = new HostelImages()
            {
                HostelId = hostelId,
                ImageUrl = imageFile
            };
            DatabaseContext db = new DatabaseContext();
            db.Add(image);
            db.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }


    public bool ValidateImageFile(IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            return false;
        }

        string[] validExtensions = { ".jpg", ".jpeg", ".png" }; // Allowed extensions
        if (!validExtensions.Contains(Path.GetExtension(imageFile.FileName).ToLower()))
        {
            return false;
        }

        return true;
    }

    public List<string> SaveImages(List<IFormFile>? imageFiles)
    {
        List<string> filePaths = new List<string>();
        int fileCount = imageFiles?.Count ?? 0;
        for (int i = 0; i < fileCount; i++)
        {
            if (imageFiles?[i] != null)
            {
                IFormFile imageFile = imageFiles[i];
                string fileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
                string filePath = Path.Combine("/Storage/Images/", fileName);
                string directoryPath = Directory.GetCurrentDirectory() + "/Storage/Images/";


                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using (FileStream fileStream = new FileStream(directoryPath + fileName, FileMode.Create, FileAccess.Write))
                {
                    imageFile.CopyTo(fileStream);
                }

                filePaths.Add(filePath);
            }
        }
        return filePaths;
    }

    public bool DeleteHostelImage(int imageId)
    {
        try
        {
            HostelImages? image = HostelImages.Query().Where(i => i.Id == imageId).FirstOrDefault();
            if (image == null)
            {
                return false;
            }
            if (image.ImageUrl != null && !image.ImageUrl.Contains("http"))
            {
                string directoryPath = Directory.GetCurrentDirectory() + image.ImageUrl;
                if (File.Exists(directoryPath))
                {
                    File.Delete(directoryPath);
                }
            }


            DatabaseContext db = new DatabaseContext();
            db.hostelImages.Remove(image);
            db.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

}