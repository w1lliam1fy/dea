const util = require('../utility');

module.exports = (client) => {
  client.on('reconnect', () => {
    util.Logger.log('Attempting to reconnect...');
  });
};
