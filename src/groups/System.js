const patron = require('patron');

class System extends patron.Group {
  constructor() {
    super({ name: 'system' });
  }
}

module.exports = new System();