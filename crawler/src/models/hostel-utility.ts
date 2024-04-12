import { DataTypes } from "sequelize";
import sequelize from "../database/database.js";
import Utility from "./utility.js";
import Hostel from "./hostel.js";

const HostelUtility = sequelize.define("hostel-utility", {
    utilityId: {
        type: DataTypes.INTEGER,
        primaryKey: true,
        allowNull: false,
        references: {
            model: Utility,
            key: 'id'
        }
    },
    hostelId: {
        type: DataTypes.INTEGER,
        primaryKey: true,
        allowNull: false,
        references: {
            model: Hostel,
            key: 'hostelId'
        }
    },
}, {
    timestamps: true,
    tableName: "hostel-utility",
    underscored: true,
});

export default HostelUtility;