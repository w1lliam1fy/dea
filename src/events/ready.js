const config = require('../config.json');

module.exports = (client) => {
  client.on('ready', async () => {
    console.log('DEA has successfully connected.');
    client.user.setGame(config.readyMessage);
  });
};
