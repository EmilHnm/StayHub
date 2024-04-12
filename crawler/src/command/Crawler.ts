import { Command } from 'commander';
import { Donga } from "./service/donga";
import { exit } from 'process';
import { DuyTan } from './service/duytan';

export class Crawler {
    private static instance: Crawler;

    private readonly services = {
        "donga": Donga,
        "duytan": DuyTan,
    };


    private constructor() {
    }

    public static getInstance(): Crawler {
        if (!Crawler.instance) {
            Crawler.instance = new Crawler();
        }

        return Crawler.instance;
    }

    public getCommand(): Command {
        const program = new Command();
        program
            .alias("crawler")
            .description("Crawl data from services supported")
            .argument("<service>", "The service to crawl data from")
            .option("-t, --type <type>", "The type of accommodation to crawl", "")
            .option("-a, --available", "Show available services")
            .action(async (service, options) => {
                if (!Object.keys(this.services).includes(service)) {
                    console.log(`Service ${service} is not supported.`);
                    this.showAvailableServices();
                    exit();
                }
                if (options.available) {
                    if (options.available) {
                        await this.showAvailableTypes(service);
                    }
                }
                if (options.type) {
                    if (await this.checkType(service, options.type)) {
                        await this.crawl(service, options.type);
                    } else {
                        console.log(`Type ${options.type} is not supported.`);
                        await this.showAvailableTypes(service);
                    }
                } else {
                    console.log(`Type is required.`);
                    await this.showAvailableTypes(service);
                }
                exit();
            });
        return program;
    }

    public async crawl(service: string, type: string | number) {
        if (!Object.keys(this.services).includes(service)) {
            throw new Error(`Service ${service} is not supported.`);
        }
        console.log(`${service} crawling started.`);
        const serviceInstance = await this.services[service].getInstance();
        await serviceInstance.crawl(type);
        console.log(`${service} crawling finished.`);
    }

    public showAvailableServices() {
        console.log("Available services:");
        for (const service of Object.keys(this.services)) {
            console.log(`- ${service}`);
        }
    }

    public async showAvailableTypes(service) {
        const serviceInstance = await this.services[service].getInstance();
        const types = serviceInstance.getAvailableType();
        console.log(`Available types for service '${service}':`);
        Object.keys(types).forEach(key => {
            console.log(`${key}: ${types[key]}`);
        });
    }

    public async checkType(service, type) {
        const serviceInstance = await this.services[service].getInstance();
        return serviceInstance.checkType(type);
    }
}