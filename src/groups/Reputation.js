const patron = require('patron.js');

class Reputation extends patron.Group {
  constructor() {
    super({ name: 'reputation' });
  }
}

module.exports = new Reputation();
