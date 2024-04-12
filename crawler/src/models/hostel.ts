import { HostileType } from './type/hostiel-type';
import { DataTypes, Model } from "sequelize";
import sequelize from "../database/database.js";
import AccommodationType from "./accommodation-type.js";
import Utility from "./utility.js";
import HostelUtility from "./hostel-utility.js";
import Host from "./host-profile.js";
import HostelImages from './hostel-images';

const Hostel = sequelize.define("Hostel", {
    id: {
        type: DataTypes.INTEGER,
        autoIncrement: true,
        primaryKey: true,
    },
    hostId: {
        type: DataTypes.INTEGER,
        references: {
            model: Host,
            key: "id",
        },
    },
    description: {
        type: DataTypes.TEXT,
        allowNull: true,
    },
    name: {
        type: DataTypes.STRING,
        allowNull: false,
    },
    address: {
        type: DataTypes.STRING,
        allowNull: false,
    },
    rooms: {
        type: DataTypes.INTEGER,
        allowNull: true,
    },
    fee: {
        type: DataTypes.INTEGER,
        allowNull: true,
    },
    source: {
        type: DataTypes.STRING,
        allowNull: true,
    },
    domain: {
        type: DataTypes.STRING,
        allowNull: true,
    },
    status: {
        type: DataTypes.ENUM("active", "inactive"),
        allowNull: false,
    },

    accommodationTypeId: {
        type: DataTypes.INTEGER,
        references: {
            model: AccommodationType,
            key: "id",
        },
    },
}, {
    timestamps: true,
    underscored: true,
    tableName: "hostels",
});

Hostel.belongsTo(AccommodationType, {
    foreignKey: "accommodationTypeId",
});

Hostel.belongsTo(Host, {
    foreignKey: "hostId",
});

Hostel.belongsToMany(Utility, {
    through: HostelUtility,
    foreignKey: "hostelId",
    otherKey: "utilityId",
});

Hostel.hasMany(HostelImages);

export default Hostel;