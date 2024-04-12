import puppeteer, { Browser, CookieParam } from 'puppeteer';
const userAgent = require('user-agents');

const navigationTimeout = 60000;

export class Browserless {
    private static instance: Browserless;
    private static browser: Browser;

    private constructor() { }

    public static async getInstance(): Promise<Browserless> {
        if (!Browserless.instance) {
            Browserless.instance = new Browserless();
        }

        await Browserless.instance.init();

        return Browserless.instance;
    }

    async init() {
        if (Browserless.browser == null) {
            console.log('Browserless instance creating');
            Browserless.browser = await puppeteer.launch({
                // headless: false,
                executablePath: '/usr/bin/google-chrome'
            });
            console.log('Browserless instance created');
        }
    }

    async close() {
        await Browserless.browser.close();
    }

    async getPage(url: string, cookie: CookieParam[] | null = null) {
        const page = await Browserless.browser.newPage();

        await page.setDefaultNavigationTimeout(navigationTimeout); // Set the navigation timeout
        await page.setUserAgent(userAgent.random().toString());
        await page.setViewport({
            width: 1920,
            height: 1080,
            deviceScaleFactor: 1,
            hasTouch: false,
            isLandscape: false,
            isMobile: false,
        });
        await page.setJavaScriptEnabled(true);
        await page.setExtraHTTPHeaders({
            'Accept-Language': 'en-US,en;q=0.9',
            'Permissions-Policy': 'interest-cohort=()'
        });

        if (cookie) {
            await page.setCookie(...cookie);
        }

        await page.goto(url);

        return page;
    }
}