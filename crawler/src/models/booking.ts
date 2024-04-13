import { DataTypes } from "sequelize";
import sequelize from "../database/database";
import RenterProfile from "./renter-profile";
import LeaseContract from "./lease-contract";
import Hostel from "./hostel";

const Booking = sequelize.define("Booking", {
    id: {
        type: DataTypes.INTEGER,
        autoIncrement: true,
        primaryKey: true,
    },
    renterId: {
        type: DataTypes.INTEGER,
        allowNull: false,
        references: {
            model: RenterProfile,
            key: "id",
        },
    },
    hostelId: {
        type: DataTypes.INTEGER,
        allowNull: false,
        references: {
            model: Hostel,
            key: "id",
        },
    },
    startDate: {
        type: DataTypes.DATE,
        allowNull: false,
    },
    endDate: {
        type: DataTypes.DATE,
        allowNull: false,
    },
    status: {
        type: DataTypes.ENUM("pending", "approved", "rejected", "cancelled"),
        allowNull: false,
        defaultValue: "pending",
    },
    leaseContractId: {
        type: DataTypes.INTEGER,
        allowNull: true,
        references: {
            model: LeaseContract,
            key: "id",
        },
    },
}, {
    timestamps: true,
    tableName: "bookings",
    underscored: true,
});

export default Booking;