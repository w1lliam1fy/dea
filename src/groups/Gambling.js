const patron = require('patron');

class Gambling extends patron.Group {
  constructor() {
    super({ name: 'gambling' });
  }
}

module.exports = new Gambling();