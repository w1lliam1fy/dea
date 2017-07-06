class StringUtil {
  static isNullOrWhiteSpace(input) {
    return (typeof input === 'undefined' || input == null) || input.replace(/\s/g, '').length === 0;
  }

  static boldify(input) {
    return '**' + input.replace(/\*|~|`/g, '').replace(/_/g, ' ') + '**';
  }
}

module.exports = StringUtil;
