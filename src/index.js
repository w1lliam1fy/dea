const path = require('path');
const patron = require('patron.js');
const discord = require('discord.js');
const db = require('./database');
const EventService = require('./services/EventService.js');
const IntervalService = require('./services/IntervalService.js');
const CommandService = require('./services/CommandService.js');
const Documentation = require('./services/Documentation.js');
const config = require('./config.json');
const credentials = require('./credentials.json');

const client = new discord.Client({ fetchAllMembers: true, messageCacheMaxSize: 5, messageCacheLifetime: 30, messageSweepInterval: 1800, disabledEvents: config.disabledEvents, restTimeOffset: 150 });

const registry = new patron.Registry();

registry.registerDefaultTypeReaders();
registry.registerGroupsIn(path.join(__dirname, 'groups'));
registry.registerCommandsIn(path.join(__dirname, 'commands'));

client.registry = Object.freeze(registry);

EventService.initiate(client);
IntervalService.initiate(client);

CommandService.run(client, new patron.Handler(registry));

initiate();

async function initiate() {
  await db.connect(credentials.mongodbConnectionURL);
  await db.userRepo.collection.updateMany({ cash: { $gt: 10000000 } }, { $set: { cash: 10000000 } });
  await client.login(credentials.token);
  await Documentation.createAndSave(registry);
}
