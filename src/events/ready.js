const config = require('../config.json');
const util = require('../utility');

module.exports = (client) => {
  client.on('ready', async () => {
    util.Logger.log('DEA has successfully connected.');
    client.user.setGame(config.readyMessage);
  });
};
