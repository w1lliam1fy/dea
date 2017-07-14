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
    const args = Array.prototype.slice.call(arguments, 1);

    return input.replace(/{(\d+)}/g, (match, number) => {
      return typeof args[number] !== 'undefined' ? args[number] : match;
    });
  }

  static alphabeticallySort(a, b) {
    return a.name.localeCompare(b.name);
  }

  static formatUsers(users) {
    let formattedMembers = '';

    for (const user of users) {
      formattedMembers += user.tag + ', ';
    }

    return formattedMembers.slice(0, -2);
  }
}

module.exports = StringUtil;
