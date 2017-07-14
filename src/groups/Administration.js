const patron = require('patron.js');
const Administrator = require('../preconditions/Administrator.js');

class Administration extends patron.Group {
  constructor() {
    super({ 
      name: 'administration',
      description: 'These commands may only be used by a user with the set mod role with a permission level of 2, the Administrator permission.',
      preconditions: [Administrator] 
    });
  }
}

module.exports = new Administration ();
