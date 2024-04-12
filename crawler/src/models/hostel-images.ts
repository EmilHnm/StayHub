import sequelize from "../database/database";
import { DataTypes, Model } from "sequelize";
import Hostel from "./hostel";

const HostelImages = sequelize.define("hostel_images", {
    id: {
        type: DataTypes.INTEGER,
        autoIncrement: true,
        primaryKey: true,
    },
    imageUrl: {
        type: DataTypes.STRING,
        allowNull: false,
    },
    hostelId: {
        type: DataTypes.INTEGER,
        references: {
            model: "hostels",
            key: "id",
        },
    },
}, {
    timestamps: true,
    underscored: true,
    tableName: "hostel_images",
});


export default HostelImages;