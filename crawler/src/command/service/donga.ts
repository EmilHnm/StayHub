import { Browser, CookieParam, Page, } from "puppeteer";
import { Browserless } from "../crawler/browserless";
import { parse } from "node-html-parser";
const he = require('he');

import { sleep } from "../lib/sleep";
import { exit } from "node:process";
import Hostel from "../../models/hostel";
import { HostileType } from "../../models/type/hostiel-type";
import Utility from "../../models/utility";
import HostelUtility from "../../models/hostel-utility";
import HostelImages from "../../models/hostel-images";
import { de } from "@faker-js/faker";
const AccommodationType = require("../../models/accommodation-type");
export class Donga {
    private static insatance: Donga;
    private static browser: Browserless;

    private readonly NAME: string = "Donga";
    private readonly URL: string = 'https://nhatro.donga.edu.vn';
    private readonly DOMAIN = 'nhatro.donga.edu.vn';
    private readonly COOKIE: CookieParam[] = [
        {
            "domain": "nhatro.donga.edu.vn",
            "httpOnly": true,
            "name": "ASP.NET_SessionId",
            "path": "/",
            "secure": false,
            "value": "0egavqwtzhvax3y4vpbofpl3"
        },
        {
            "domain": "nhatro.donga.edu.vn",
            "httpOnly": true,
            "name": "__RequestVerificationToken",
            "path": "/",
            "secure": false,
            "value": "P8x3th9lJy2JEOiV3EDDsxnFIVBQEijcNXohlnMMKM6I1Y7sR49OBNrPmqQVsoBnVBK4A-TH5IKeav2r6FD36bq9cFo92QNiDRntP2CfX5s1"
        },
        {
            "domain": "nhatro.donga.edu.vn",
            "httpOnly": false,
            "name": "SL_G_WPT_TO",
            "path": "/",
            "secure": true,
            "value": "vi"
        },
        {
            "domain": "nhatro.donga.edu.vn",
            "httpOnly": false,
            "name": "SL_GWPT_Show_Hide_tmp",
            "path": "/",
            "secure": true,
            "value": "1"
        },
        {
            "domain": "nhatro.donga.edu.vn",
            "httpOnly": false,
            "name": "SL_wptGlobTipTmp",
            "path": "/",
            "secure": true,
            "value": "1"
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
        3: 3,
        4: 4,
        5: 5,
    };

    private constructor() {

    }

    public static async getInstance(): Promise<Donga> {
        if (!Donga.insatance) {
            Donga.insatance = new Donga();
        }
        if (!Donga.browser) {
            Donga.browser = await Browserless.getInstance();
        }
        return Donga.insatance;
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
        const accommodationType = await AccommodationType.findOne({ where: { id: type } });
        do {
            let endpoint = `${this.URL}/nhatro/timkiem?loai=${type}&page=${page}`;
            console.log(endpoint);
            let PageReq = await Donga.browser.getPage(endpoint, this.COOKIE);
            await sleep(5000);
            let result = await PageReq.content();

            if (result === null) {
                console.log(`   Error when fetching page ${page}`);
                console.log(`   URL ${endpoint}`);
                canContinue = false;
                continue;
            }
            const root = parse(result);
            const listRoomElements = root.querySelectorAll("body > div > div.container-xxl.py-5 > div > div.tab-content > div > div");

            if (listRoomElements === null || listRoomElements.length === 0) {
                canContinue = false;
                console.log(`   No room found on page ${page}`);
                console.log(`   URL ${endpoint}`);
                continue;
            }

            await Promise.all(listRoomElements.map(async (element) => {
                const nameStr = element.querySelector("div > div.p-4.pb-0 > a").innerText;
                const url = element.querySelector("div > div.p-4.pb-0 > a").getAttribute("href");
                const addressStr = element.querySelector("div > div.p-4.pb-0 > p").innerText;
                const feeStr = element.querySelector("div > div.p-4.pb-0 > h5").innerText.replace(/[^0-9.]/g, "");
                let fee = parseFloat(feeStr);
                if (isNaN(fee)) {
                    fee = 0;
                } else {
                    fee = Math.abs(fee) * 1000000;
                }
                let name = he.decode(nameStr);
                let address = he.decode(addressStr);

                const oldData = await Hostel.findOne({ where: { source: url } });
                if (oldData !== null) {
                    console.log(`      Room ${name} already exists`);
                    await oldData.update({
                        name: name,
                        address: address,
                        accommodationTypeId: accommodationType.id,
                        fee: fee,
                    });
                    return;
                } else {
                    try {

                        let hostel = await Hostel.create({
                            hostId: 1,
                            name: name,
                            address: address,
                            fee: fee,
                            source: url,
                            status: "active",
                            domain: this.DOMAIN,
                            accommodationTypeId: accommodationType.id,
                        });
                        if (hostel) {
                            console.log("   Crawled raw infomation:");
                            console.log(`      Name: ${name}`);
                            console.log(`      Address: ${address}`);
                            console.log(`      Fee: ${fee}`);
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
            }));

            page++;
            await sleep(3000);
            PageReq.close();
        } while (canContinue);

        return;
    }

    public async getRoomDetail(type: number): Promise<any> {
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
                let PageReq = await Donga.browser.getPage(endpoint, this.COOKIE);
                let result = await PageReq.content();

                if (result === null) {
                    console.log(`   Error when fetching page ${endpoint}`);
                    return;
                } else {
                    const root = parse(result);
                    const rooms = await this.parseRoom(root, (hostels[i] as any).id);
                    console.log(`   Added ${rooms} rooms`)
                    const utility = await this.parseUtility(root, (hostels[i] as any).id);
                    console.log(`   Added ${utility} utilities`)
                    const description = await this.parseDescription(root, (hostels[i] as any).id);
                    console.log(`   Added Description: ${description}`);
                    const imageCount = await this.parseImage(root, (hostels[i] as any).id);
                    console.log(`   Added ${imageCount} images`);
                }
                await PageReq.close();
            } catch (err) {
                console.log(`   Error when fetching page ${endpoint}`);
                continue;
            }
        }
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


    public async crawl(type) {
        type = this.MAPPING_TYPE[type];
        await this.importHostelType();
        await this.getListRoom(type);
        console.log("Start crawl room");
        await this.getRoomDetail(type);
        console.log("Start remove error hostel");
        await this.removeErrorHostel(type);
        console.log("Cleaning");
        Donga.browser.close();

        return;
    }


    public getAvailableType() {
        return this.TYPE;
    }

    public checkType(type: string): boolean {
        return Object.keys(this.TYPE).includes(type);
    }

    private async parseUtility(root: any, id: number): Promise<number> {
        const utinityElements = root.querySelectorAll("body > div.container-xxl.bg-white.p-0 > div:nth-child(14) > div > div > div > div > div > div.row > div");
        await Promise.all(utinityElements.map(async (element) => {
            let name = element.innerText;
            name = he.decode(name);
            name = name.trim();
            if (name === "") {
                return;
            }
            console.log(`      Utility: ${name}`);
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
        return utinityElements.length;
    }

    private async parseDescription(root: any, id: number): Promise<string> {
        const managementMechanismStr = root.querySelector('body > div.container-xxl.bg-white.p-0 > div:nth-child(14) > div > div > div > div > div > div.mb-4 > div > div:nth-child(2) > p');
        let managementMechanism = "";
        if (managementMechanismStr !== null) {
            managementMechanism = he.decode(managementMechanismStr.innerText);
        }
        const statusStr = root.querySelector('body > div.container-xxl.bg-white.p-0 > div:nth-child(14) > div > div > div > div > div > div.mb-4 > div > div:nth-child(3) > p');
        let hostelsStatus = "";
        if (statusStr !== null) {
            hostelsStatus = he.decode(statusStr.innerText);
        }
        const peoplePerRoomStr = root.querySelector('body > div.container-xxl.bg-white.p-0 > div:nth-child(14) > div > div > div > div > div > div.mb-4 > div > div:nth-child(5) > p');
        let peoplePerRoom = "";
        if (peoplePerRoomStr !== null) {
            peoplePerRoom = he.decode(peoplePerRoomStr.innerText);
        }
        const usageStr = root.querySelector('body > div.container-xxl.bg-white.p-0 > div:nth-child(14) > div > div > div > div > div > div.mb-4 > div > div:nth-child(6) > p');
        let usage = "";
        if (usageStr !== null) {
            usage = he.decode(usageStr.innerText);
        }
        const busStr = root.querySelector('body > div.container-xxl.bg-white.p-0 > div:nth-child(14) > div > div > div > div > div > div.mb-4 > div > div:nth-child(7) > p');
        let bus = "";
        if (busStr !== null) {
            bus = he.decode(busStr.innerText);
        }
        const descriptionRaw = [managementMechanism, hostelsStatus, peoplePerRoom, usage, bus];
        const description = descriptionRaw.join("\n");

        await Hostel.update({ description: description }, { where: { id: id } });

        return description;
    }

    private async parseRoom(root: any, id: number): Promise<number> {
        const roomsStr = root.querySelector("body > div.container-xxl.bg-white.p-0 > div:nth-child(14) > div > div > div > div > div > div.mb-4 > div > div:nth-child(4) > p");
        let room = parseInt(roomsStr.innerText.replace(/[^0-9.]/g, ""));
        await Hostel.update({ rooms: room }, { where: { id: id } });
        return room;
    }

    private async parseImage(root: any, id: number): Promise<number> {
        let imageUrl: string[] = [];
        const imageElements = root.querySelectorAll('body > div.container-xxl.bg-white.p-0 > div.photo-gallery > div > div.row.photos > div > a > img');
        for (let i = 0; i < imageElements.length; i++) {
            const image = imageElements[i];
            const imageSrc = `${this.URL}${image.getAttribute('src')}`;
            try {
                const response = await fetch(imageSrc);
                if (response.ok) {
                    imageUrl.push(imageSrc)
                }
            } catch (err) {
                console.log(`   Error when fetching image ${imageSrc}`);
            }
        }
        for (let i = 0; i < imageUrl.length; i++) {
            let image = await HostelImages.findOne({ where: { url: imageUrl[i], hostelId: id } });
            if (image !== null) {
                continue;
            } else {
                await HostelImages.create({
                    imageUrl: imageUrl[i],
                    hostelId: id
                });
                console.log(`      Image ${imageUrl[i]} added`);
            }
        }
        return imageUrl.length;
    }

}