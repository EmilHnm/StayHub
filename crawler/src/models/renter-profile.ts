import { DataTypes } from "sequelize";
import sequelize from "../database/database.js";
import User from "./user.js";

const RenterProfile = sequelize.define("renter-profile", {
    id: {
        type: DataTypes.INTEGER,
        autoIncrement: true,
        primaryKey: true,
    },
    userId: {
        type: DataTypes.INTEGER,
        references: {
            model: User,
            key: "id",
        },
    },
    fullname: {
        type: DataTypes.STRING,
    },
    gender: {
        type: DataTypes.STRING,
    },
    phone: {
        type: DataTypes.STRING,
    },
    address: {
        type: DataTypes.STRING,
    },
    identityCardNumber: {
        type: DataTypes.STRING,
    },
}, {
    timestamps: true,
    tableName: "renter-profile",
    underscored: true,
});

export default RenterProfile;

module.exports = RenterProfile;