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

  static hoursToMs(input) {
    return input * 3600000;
  }

  static daysToMs(input) {
    return input * 86400000;
  }
}

module.exports = NumberUtil;
