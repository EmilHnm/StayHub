import { Command } from "commander";
import { DB } from "./DB.js";
import { AccountDB } from "./AccountDB.js";
import { Crawler } from "./Crawler.js";

type command = {
    [key: string]: any;
};

export class CommandType {
    #commandType: command = {
        "db": DB.getInstance().getCommand(),
        "ac": AccountDB.getInstance().getCommand(),
        "cr": Crawler.getInstance().getCommand(),
    };

    public getCommands() {
        return this.#commandType;
    }
};