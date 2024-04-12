import { CookieParam } from "puppeteer";
import { Browserless } from "../crawler/browserless";
import AccommodationType from "../../models/accommodation-type";
import Hostel from "../../models/hostel";
import { sleep } from "../lib/sleep";
import parse from "node-html-parser";
import { Console } from "node:console";
import HostelUtility from "../../models/hostel-utility";
import Utility from "../../models/utility";
import HostelImages from "../../models/hostel-images";


export class DuyTan {
    private static instance: DuyTan;
    private static browser: Browserless;
    private readonly NAME: string = "DuyTan";
    private readonly URL: string = "https://nhatro.duytan.edu.vn/";
    private readonly DOMAIN: string = "nhatro.duytan.edu.vn";
    private readonly COOKIE: CookieParam[] = [
        {
            "domain": "nhatro.duytan.edu.vn",
            "httpOnly": false,
            "name": "SL_G_WPT_TO",
            "path": "/",
            "secure": true,
            "value": "vi"
        },
        {
            "domain": "nhatro.duytan.edu.vn",
            "httpOnly": false,
            "name": "SL_GWPT_Show_Hide_tmp",
            "path": "/",
            "secure": true,
            "value": "1"
        },
        {
            "domain": "nhatro.duytan.edu.vn",
            "httpOnly": false,
            "name": "SL_wptGlobTipTmp",
            "path": "/",
            "secure": true,
            "value": "1"
        },
        {
            "domain": "nhatro.duytan.edu.vn",
            "httpOnly": true,
            "name": "ASP.NET_SessionId",
            "path": "/",
            "secure": false,
            "value": "om3mxzbjqudyb35buc20nouo"
        }
    ];

    private readonly TYPE = {
        1: "Nhà trọ và phòng trọ",
        2: "Nhà nguyên căn",
        3: "Nhà tập thể",
        4: "Ký túc xá",
        5: "Chung cư mini",
    };

    private readonly MAPPING_TYPE = {
        1: 1,
        2: 2,
        3: 6,
        4: 5,
        5: 3,
    };

    private constructor() { }
    public static async getInstance(): Promise<DuyTan> {
        if (!DuyTan.instance) {
            DuyTan.instance = new DuyTan();
        }
        if (!DuyTan.browser)
            DuyTan.browser = await Browserless.getInstance();
        return DuyTan.instance;
    }

    public async importHostelType(): Promise<any> {
        console.log(`Create accommodation type if not exist`);
        await Promise.all(Object.keys(this.TYPE).map(async (key) => {
            let accommodation_type = await AccommodationType.findOne({ where: { id: key } });
            if (accommodation_type === null) {
                console.log(`   Creating new type ${this.TYPE[key]}`);
                await AccommodationType.create({
                    id: key,
                    name: this.TYPE[key],
                });
            }
        }))
    }

    public async getListRoom(type): Promise<void> {
        console.log("   Getting list room");
        let page = 1;
        let canContinue = true;
        let realType = this.MAPPING_TYPE[type];
        const accommodationType = await AccommodationType.findOne({ where: { id: type } }) as any;
        do {
            let endpoint = `${this.URL}/nha-tro/tim-phong-tro/?loaiNhaTro=${realType}&page=${page}`;
            console.log(endpoint);
            let PageReq = await DuyTan.browser.getPage(endpoint, this.COOKIE);
            await sleep(5000);
            let result = await PageReq.content();
            await PageReq.close();
            if (result === null) {
                console.log(`   Error when fetching page ${page}`);
                console.log(`   URL ${endpoint}`);
                canContinue = false;
                continue;
            }
            const root = parse(result);
            const listRoomElements = root.querySelectorAll("body > div > div.content-session > div > div > div.box-new > div > div");

            if (listRoomElements === null || listRoomElements.length === 0) {
                canContinue = false;
                console.log(`   No room found on page ${page}`);
                console.log(`   URL ${endpoint}`);
                continue;
            }

            console.log(`   Found ${listRoomElements.length} rooms on page ${page}`);

            await Promise.all(listRoomElements.map(async (roomElement) => {
                const nameStr = roomElement.querySelector(' div > div.box-room > h3 > a').innerText;
                const name = nameStr.trim();
                let priceStr = roomElement.querySelector('div > div.box-room > div.book-room > p').innerText;
                priceStr = priceStr.trim();
                let priceNumber = priceStr.match(/(?:-\s*)?(\d+(?:\.\d+)?)/g);
                if (!priceNumber) return;
                priceStr = priceNumber[priceNumber.length - 1];
                const price = parseFloat(priceStr.replace("-", "").trim()) * 1000000;
                let addressStr = roomElement.querySelector('div > div.box-room > div.diachiroom > div').innerText;
                const address = addressStr.trim();
                const url = roomElement.querySelector(" div > div.box-room > h3 > a").getAttribute("href");
                console.log(`   Found room ${name} with price ${price} at ${address}\n ${url}`);

                const oldData = await Hostel.findOne({ where: { source: url } });
                if (oldData !== null) {
                    console.log(`      Room ${name} already exists`);
                    await oldData.update({
                        name: name,
                        address: address,
                        accommodationTypeId: accommodationType.id,
                        fee: price,
                    });
                    return;
                } else {
                    try {
                        let hostel = await Hostel.create({
                            hostId: 1,
                            name: name,
                            address: address,
                            fee: price,
                            source: url,
                            status: "active",
                            domain: this.DOMAIN,
                            accommodationTypeId: accommodationType.id,
                        });
                        if (hostel) {
                            console.log("   Crawled raw infomation:");
                            console.log(`      Name: ${name}`);
                            console.log(`      Address: ${address}`);
                            console.log(`      Fee: ${price}`);
                            console.log(`      Url: ${url}`);
                            return;
                        } else {
                            console.log("   Error when creating hostel");
                            return;
                        }
                    } catch (err) {
                        console.log(err);
                    }
                }

                return;
            }));


            page++;
            await sleep(3000);
            PageReq.close();
        } while (canContinue);
        return;
    }

    public async getRoomDetail(type: number) {
        console.log("   Getting room detail");
        const hostels = await Hostel.findAll({
            where: {
                domain: this.DOMAIN,
                description: null,
                accommodationTypeId: type,
            }
        });
        for (let i = 0; i < hostels.length; i++) {
            let endpoint = `${this.URL}${(hostels[i] as any).source}`;
            console.log(`   Fetching page ${endpoint}`);
            try {
                let PageReq = await DuyTan.browser.getPage(endpoint, this.COOKIE);
                let result = await PageReq.content();
                await PageReq.close();
                if (result === null) {
                    console.log(`   Error when fetching page ${endpoint}`);
                    return;
                } else {
                    const root = parse(result);
                    const rooms = await this.parseRoom(root, (hostels[i] as any).id);
                    console.log(`   Found ${rooms} rooms`);
                    const utilities = await this.parseUtilities(root, (hostels[i] as any).id);
                    console.log(`   Found ${utilities} utilities`);
                    const image = await this.parseImage(root, (hostels[i] as any).id);
                    console.log(`   Found ${image} images`);
                    const description = await this.parseDescription(root, (hostels[i] as any).id);
                    console.log(`   Found ${description} descriptions`);

                }
            } catch (err) {
                console.log(`   Error when fetching page ${endpoint}`);
                console.log(err);
                continue;
            }
        }
    }

    public async crawl(type) {

        await this.importHostelType();
        await this.getListRoom(type);
        console.log("Start crawl room");
        await this.getRoomDetail(type);
        console.log("Start remove error hostel");
        await this.removeErrorHostel(type);
        return;
    }

    public async removeErrorHostel(type: number): Promise<boolean> {
        try {
            const hostels = await Hostel.findAll({
                where: {
                    domain: this.DOMAIN,
                    description: null,
                    accommodationTypeId: type,
                }
            });
            for (let i = 0; i < hostels.length; i++) {
                await Hostel.destroy({ where: { id: (hostels[i] as any).id } });
            }
            console.log(`   Removed ${hostels.length} error hostels`);
            return true;
        } catch (err) {
            console.log(err);
        }
        return false;
    }

    private async parseRoom(root: any, id: number): Promise<number> {
        const roomRaw = root.querySelector("body > div > div.content-session > div > div.container > div > div.col-lg-8 > div:nth-child(2) > div:nth-child(3) > ul > li.info_room_ps");
        let room = 1;
        if (roomRaw === null) {
            console.log("   Can not find room detail");
        } else {
            const roomText = roomRaw.innerText.replace(/[^0-9.]/g, "").trim();
            room = parseInt(roomText);
            if (isNaN(room)) {
                room = 1;
            }
        }
        await Hostel.update({ rooms: room }, { where: { id: id } });
        return room;
    }

    private async parseUtilities(root: any, id: number): Promise<number> {
        const utilitiesRaw = root.querySelectorAll("body > div > div.content-session > div > div.container > div > div.col-lg-8 > div:nth-child(2) > div:nth-child(3) > ul > li");
        if (utilitiesRaw === null) {
            console.log("   Can not find utilities");
            return 0;
        } else {
            let utilities = [];
            utilitiesRaw.map((utility) => {
                if (!utility.classList.contains("info_room_ps")) {
                    utilities.push(utility.innerText.trim());
                }
            });
            await Promise.all(utilities.map(async (name) => {
                let utility = await Utility.findOne({ where: { name: name } })
                if (utility === null) {
                    utility = await Utility.create({
                        name: name,
                    });
                }
                if (utility && await HostelUtility.findOne({ where: { hostelId: id, utilityId: (utility as any).id } }) === null) {
                    await HostelUtility.create({
                        hostelId: id,
                        utilityId: (utility as any).id
                    });
                }
                return;
            }));
            return utilities.length;
        }
    }

    private async parseImage(root: any, id: number): Promise<number> {
        let imageUrl: string[] = [];
        const imageContainer = root.querySelectorAll('#slider-zoom > div.slide_img > div > div.owl-stage-outer > div > div');
        for (let i = 0; i < imageContainer.length; i++) {
            const image = imageContainer[i].querySelector('img');
            if (image === null) {
                continue;
            }
            const imageSrc = `${this.URL}${image.getAttribute('src')} `;
            try {
                const response = await fetch(imageSrc);
                if (response.ok) {
                    imageUrl.push(imageSrc);
                    console.log(`      Image ${imageSrc} added`);
                } else {
                    console.log(`      Image ${imageSrc} not found`);
                }
            } catch (err) {
                console.log(`   Error when fetching image ${imageSrc} `);
                if (err.message === "fetch is not defined") {
                    console.log(`   Please use Node.js versions 17 or later`);
                }
            }
        }
        for (let i = 0; i < imageUrl.length; i++) {
            let image = await HostelImages.findOne({ where: { image_url: imageUrl[i], hostelId: id } });
            if (image !== null) {
                continue;
            } else {
                const image = await HostelImages.create({
                    imageUrl: imageUrl[i],
                    hostelId: id
                });
                if (image !== null) {
                    console.log(`      Image ${imageUrl[i]} added`);
                }
            }
        }
        return imageUrl.length;
    }

    private async parseDescription(root: any, id: number): Promise<string> {
        const descriptionRaw = root.querySelector('body > div > div.content-session > div > div.container > div > div.col-lg-8 > div:nth-child(3) > div');
        let description = descriptionRaw.innerText.trim();
        await Hostel.update({ description: description }, { where: { id: id } });

        return description;

    }

    public getAvailableType() {
        return this.TYPE;
    }

    public checkType(type: string): boolean {
        return Object.keys(this.TYPE).includes(type);
    }

}