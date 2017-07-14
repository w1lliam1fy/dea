const patron = require('patron.js');
const Moderator = require('../preconditions/Moderator.js');

class Moderation extends patron.Group {
  constructor() {
    super({
      name: 'moderation',
      description: 'These commands may only be used by a user with the set mod role with a permission level of 1, or the Administrator permission.',
      preconditions: [Moderator]
    });
  }
}

module.exports = new Moderation ();
