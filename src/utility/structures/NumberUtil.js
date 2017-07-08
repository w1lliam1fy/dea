const formatter = Intl.NumberFormat('en-US', {
  style: 'currency',
  currency: 'USD',
  minimumFractionDigits: 2,
});

class NumberUtil {
  static USD(input) {
    return formatter.format(input);
  }

  static realValue(input) {
    return input / 100;
  }

  static format(input) {
    return this.USD(input / 100);
  }

  static isEven(input) {
    return input % 2 == 0;
  }

  static hoursToMs(input) {
    return input * 3600000;
  }

  static daysToMs(input) {
    return input * 86400000;
  }
  
  static msToTime(input) {
    return {
      milliseconds: parseInt((input % 1000) / 100),
      seconds: parseInt((input / 1000) % 60),
      minutes: parseInt((input / (1000 * 60)) % 60),
      hours: parseInt((input / (1000 * 60 * 60)) % 24),
      days: parseInt(input / (1000 * 60 * 60 * 24))
    };
  }
}

module.exports = NumberUtil;
