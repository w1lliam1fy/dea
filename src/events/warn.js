module.exports = (client) => {
  client.on('warn', console.warn);
};
