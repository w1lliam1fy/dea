const patron = require('patron.js');
const Owner = require('../preconditions/Owner.js');

class Owners extends patron.Group {
  constructor() {
    super({ name: 'owners', preconditions: [Owner] });
  }
}

module.exports = new Owners();
