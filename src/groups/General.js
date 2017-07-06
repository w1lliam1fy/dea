const patron = require('patron');

class General extends patron.Group {
  constructor() {
    super({ name: 'general' });
  }
}

module.exports = new General();