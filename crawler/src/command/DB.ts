import { Command } from "commander";
import sequelize from "../database/database";

const Utility = require('../models/utility');
const AccommodationType = require('../models/accommodation-type');
const User = require('../models/user');
const Hostel = require('../models/hostel');
const HostProfile = require('../models/host-profile');
const RenterProfile = require('../models/renter-profile');
const HostelUtility = require('../models/hostel-utility');
const Booking = require('../models/booking');
const LeaseContract = require('../models/lease-contract');
const HostelImages = require('../models/hostel-images');

export class DB {
    private static instance: DB;

    private constructor() {
    }

    public static getInstance(): DB {
        if (!DB.instance) {
            DB.instance = new DB();
        }
        return DB.instance;
    }


    public getCommand(): Command {
        const program = new Command();
        program
            .alias("database")
            .description("Database operations")
            .argument("<name>", "The database operation to perform")
            .option("--admin", "Create admin account after sync/refresh database if not exists")
            .action((name: "synchronize" | "refresh" | "delete", options) => {
                switch (name) {
                    case "synchronize":
                        this.synchronizeDatabase(options);
                        break;

                    case "refresh":
                        this.refreshDatabase(options);
                        break;

                    case "delete":
                        this.deleteDatabase();
                        break;

                    default:
                        break;
                }
            });

        return program;
    }

    private synchronizeDatabase(options) {
        sequelize.sync().then(() => {
            if (options.admin) {
                if (!User.findOne({ where: { role: "admin" } })) {
                    console.log("Admin account already exists");
                    return;
                }
                const admin = new User({
                    name: "Admin",
                    email: "admin@example.com",
                    password: "admin",
                    role: "Admin",
                });
                admin.save().then(() => {
                    console.log("Admin account created");
                    console.log(`${admin.name} ${admin.email} ${admin.password}`);
                });
            }
        });
    }

    private refreshDatabase(options) {
        sequelize.sync({ force: true }).then(() => {
            if (options.admin) {
                const admin = new User({
                    name: "Admin",
                    email: "admin@example.com",
                    password: "admin",
                    role: "Admin",
                });
                admin.save().then(() => {
                    console.log("Admin account created");
                    console.log(`${admin.name} ${admin.email} ${admin.password}`);
                });
            }
        });
    }

    private deleteDatabase() {
        sequelize.drop().then(() => {
            console.log("Database deleted");
        });
    }
};