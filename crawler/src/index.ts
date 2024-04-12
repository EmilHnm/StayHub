import { Command, program } from "commander";
import { CommandType } from "./command/CommandType.js";

program
    .version("1.0.0")
    .description("A simple CLI application built with Node.js");

const commands = new CommandType();


Object.values(commands.getCommands()).forEach((command: Command, index: number) => {
    program.addCommand(command.name(Object.keys(commands.getCommands())[index]));
});

program.parse(process.argv);