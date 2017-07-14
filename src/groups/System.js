const patron = require('patron.js');

class System extends patron.Group {
  constructor() {
    super({
      name: 'system'
    });
  }
}

module.exports = new System();
