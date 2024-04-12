import { Command } from "commander";
const User = require("../models/user");
const RenterProfile = require("../models/renter-profile");
const HostProfile = require("../models/host-profile");
import { faker } from "../database/faker";
import { SexType } from "@faker-js/faker";
export class AccountDB {
    private static instance: AccountDB;

    private constructor() {
        // ...
    }

    public static getInstance(): AccountDB {
        if (!this.instance) {
            this.instance = new AccountDB();
        }
        return this.instance;
    }

    public getCommand(): Command {
        const command = new Command();
        command
            .alias("accountdb")
            .description("Account database operations")
            .argument("<email>", "email of the user to operate on")
            .argument("<password>", "Password of the user to operate on")
            .option("--admin", "Save user as admin")
            .option("--host-profile", "Create a fake host profile for user")
            .option("--renter-profile", "Create a fake renter profile for user")
            .option("-f, --force", "Force the operation")
            .action(async (email, password, options) => {
                if (!email || !password) {
                    console.log("Please provide email and password");
                    return;
                }
                let user: typeof User | null = null;
                if (await User.findOne({ where: { email: email } }) !== null) {
                    if (options.force) {
                        user = await User.findOne({ email });
                        console.log("User overwritten");
                    } else {
                        console.warn("User already exists");
                        console.log("Use --force to overwrite or provide a different email");
                        return;
                    }
                } else {
                    user = new User({
                        email,
                        password,
                        admin: options.admin,
                        role: options.admin ? "Admin" : "User",
                    });
                    await user.save();
                    console.log("User created successfully");
                }
                if (user) {
                    const fakeGender = faker.person.gender() as SexType;
                    const fakeName = faker.person.fullName({ sex: fakeGender });

                    if (options.hostProfile) {
                        if (await HostProfile.findOne({ userId: user.id }) !== null) {
                            if (options.force) {
                                const oldHostProfile = await HostProfile.findOne({ userId: user.id });
                                await oldHostProfile.destroy();
                                console.log("Host profile overwritten");
                            } else {
                                console.warn("Host profile already exists");
                                console.log("Use --force to overwrite");
                                return;
                            }
                        }
                        const hostProfile = new HostProfile({
                            userId: user.id,
                            fullname: fakeName,
                            phone: faker.phone.number(),
                        });
                        await hostProfile.save();
                        console.log(`Host profile created successfully with name ${hostProfile.fullname}`);
                    }

                    if (options.renterProfile) {
                        if (await RenterProfile.findOne({ userId: user.id }) !== null) {
                            if (options.force) {
                                const oldRenterProfile = await RenterProfile.findOne({ userId: user.id });
                                await oldRenterProfile.destroy();
                                console.log("Renter profile overwritten");
                            } else {
                                console.warn("Renter profile already exists");
                                console.log("Use --force to overwrite");
                                return;
                            }
                        }
                        const renterProfile = new RenterProfile({
                            userId: user.id,
                            fullname: fakeName,
                            phone: faker.phone.number(),
                            address: faker.location.city(),
                            gender: fakeGender,
                            identityCardNumber: faker.string.numeric("####-####-####")
                        });
                        await renterProfile.save();
                        console.log(`Renter profile created successfully with name ${renterProfile.fullname}`);
                    }
                } else {
                    console.log("Error creating user");
                }
            });
        return command;
    }
}