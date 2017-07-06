const patron = require('patron');

class Crime extends patron.Group {
  constructor() {
    super({ name: 'crime' });
  }
}

module.exports = new Crime();