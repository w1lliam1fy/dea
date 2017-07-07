const patron = require('patron.js');
const Administrator = require('../preconditions/Administrator.js');

class Administration extends patron.Group {
  constructor() {
    super({ name: 'administration', preconditions: [Administrator] });
  }
}

module.exports = new Administration ();
