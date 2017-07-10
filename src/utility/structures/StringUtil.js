class StringUtil {
  static isNullOrWhiteSpace(input) {
    return !input || input.replace(/\s/g, '').length === 0;
  }

  static boldify(input) {
    return '**' + input.replace(/\*|~|`/g, '').replace(/_/g, ' ') + '**';
  }

  static upperFirstChar(input) {
    return input.charAt(0).toUpperCase() + input.slice(1);
  }

  static format(input) {
    const args = arguments;

    return input.replace(/(\{\{\d\}\}|\{\d\})/g, function (b) {
      if (input.substring(0, 2) == '{{') {
        return input;
      } 

      const c = parseInt(input.match(/\d/)[0]);
      return args[c + 1];
    });
  }

  static alphabeticallySort(a, b) {
    return a.name.localeCompare(b.name);
  }
}

module.exports = StringUtil;
