import { DataTypes } from "sequelize";
import sequelize from "../database/database";
import RenterProfile from "./renter-profile";
import Hostel from "./hostel";

const LeaseContract = sequelize.define("LeaseContract", {
    id: {
        type: DataTypes.INTEGER,
        autoIncrement: true,
        primaryKey: true,
    },
    renterId: {
        type: DataTypes.INTEGER,
        references: {
            model: RenterProfile,
            key: "id",
        },
    },
    hostelId: {
        type: DataTypes.INTEGER,
        references: {
            model: Hostel,
            key: "id",
        },
    },
    startDate: {
        type: DataTypes.DATE,
    },
    endDate: {
        type: DataTypes.DATE,
    },
    deposit: {
        type: DataTypes.INTEGER,
    },
    monthlyRent: {
        type: DataTypes.INTEGER,
    },
    status: {
        type: DataTypes.STRING,
        values: ['active', 'inactive'],
        defaultValue: 'active',
    },
}, {
    tableName: 'lease-contract',
    timestamps: true,
    underscored: true,
});

export default LeaseContract;