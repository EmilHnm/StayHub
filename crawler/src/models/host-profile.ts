import { DataTypes } from "sequelize";
import sequelize from "../database/database.js";
import Hostel from "./hostel.js";
import User from "./user.js";

const HostProfile = sequelize.define("HostProfile", {
    id: {
        type: DataTypes.INTEGER,
        autoIncrement: true,
        primaryKey: true,
    },
    userId: {
        type: DataTypes.INTEGER,
        allowNull: false,
        references: {
            model: User,
            key: "id",
        },
    },
    fullname: {
        type: DataTypes.STRING,
        allowNull: false,
    },
    phone: {
        type: DataTypes.STRING,
        allowNull: false,
    },
}, {
    timestamps: true,
    underscored: true,
    tableName: "hosts-profiles",
});

HostProfile.belongsTo(User, {
    foreignKey: "userId",
    as: "user",
});

export default HostProfile;

module.exports = HostProfile;