const NumberUtil = require('./NumberUtil.js');

class Logger {
  static log(message) {
    const date = new Date();
    console.log(NumberUtil.pad(date.getHours(), 2) + ':'  +NumberUtil.pad(date.getMinutes(), 2) + ':' + NumberUtil.pad(date.getSeconds(), 2) + ' ' + message);
  }
}

module.exports = Logger;
