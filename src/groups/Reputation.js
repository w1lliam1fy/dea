const patron = require('patron.js');

class Reputation extends patron.Group {
  constructor() {
    super({
      name: 'reputation',
      description: 'The repuration group has been added in order to show you have gained a certain status and respect in a server, which will allow you access to more commands and features with a higher reputation.'
    });
  }
}

module.exports = new Reputation();
