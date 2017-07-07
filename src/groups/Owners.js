const patron = require('patron.js');

class Owners extends patron.Group {
  constructor() {
    super({ name: 'owners', preconditions: [Owners] });
  }
}

module.exports = new Owners();
