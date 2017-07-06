module.exports = (client) => {
  client.on('reconnect', () => {
    console.log('DEA has disconnected.');
  });
};
