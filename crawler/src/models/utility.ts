import { DataTypes } from "sequelize";
import sequelize from "../database/database.js";

const Utility = sequelize.define("utilities", {
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
    description: {
        type: DataTypes.STRING,
        allowNull: true,
    },
}, {
    timestamps: true,
    underscored: true,
    tableName: "utilities",
});


export default Utility;