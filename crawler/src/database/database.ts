import { Dialect, Sequelize } from "sequelize";
import config from "../config/database.js";

const sequelize = new Sequelize({
  dialect: config.dialect as Dialect,
  database: config.database,
  username: config.username,
  password: config.password,
  host: config.host,
  port: config.port,
  logging: false,
})

export default sequelize;