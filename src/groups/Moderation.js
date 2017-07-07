const patron = require('patron.js');
const Moderator = require('../preconditions/Moderator.js');

class Moderation extends patron.Group {
  constructor() {
    super({ name: 'moderation', preconditions: [Moderator] });
  }
}

module.exports = new Moderation ();
