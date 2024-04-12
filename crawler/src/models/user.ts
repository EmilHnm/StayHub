import { DataTypes } from "sequelize";
import sequelize from "../database/database.js";

const User = sequelize.define("User", {
    id: {
        type: DataTypes.INTEGER,
        autoIncrement: true,
        primaryKey: true,
    },
    email: {
        type: DataTypes.STRING,
        allowNull: false,
    },
    password: {
        type: DataTypes.STRING,
        allowNull: false,
    },
    role: {
        type: DataTypes.ENUM("Admin", "User"),
        defaultValue: "User",
    },
}, {
    timestamps: true,
    underscored: true,
    tableName: "users",
});

export default User;

module.exports = User;