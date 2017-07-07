const path = require('path');
const patron = require('patron.js');
const discord = require('discord.js');
const db = require('./database');
const EventService = require('./services/EventService.js');
const CommandService = require('./services/CommandService.js');
const config = require('./config.json');
const credentials = require('./credentials_beta.json');

const client = new discord.Client({ fetchAllMembers: true, messageCacheMaxSize: 5, messageCacheLifetime: 10, messageSweepInterval:1800, disabledEvents: config.disabledEvents, restTimeOffset: 150 });

const registry = new patron.Registry();

registry.registerDefaultTypeReaders();
registry.registerGroupsIn(path.join(__dirname, 'groups'));
registry.registerCommandsIn(path.join(__dirname, 'commands'));

EventService.initiate(client);

CommandService.run(client, new patron.Handler(registry));

db.connect(credentials.mongodbConnectionUrl)
  .then(() => {
    client.login(credentials.token);
  });
