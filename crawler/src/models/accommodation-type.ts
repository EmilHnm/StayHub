import { DataTypes } from "sequelize";
import sequelize from "../database/database.js";
import Hostel from "./hostel.js";

const AccommodationType = sequelize.define("AccommodationType", {
    id: {
        type: DataTypes.INTEGER,
        allowNull: false,
        autoIncrement: true,
        primaryKey: true,
    },
    name: {
        type: DataTypes.STRING,
        allowNull: false,
    },
}, {
    timestamps: true,
    underscored: true,
    tableName: "accommodation-types",
});


export default AccommodationType;

module.exports = AccommodationType;