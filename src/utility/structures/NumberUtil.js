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
}

module.exports = NumberUtil;
