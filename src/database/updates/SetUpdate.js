class SetUpdate {
  constructor(property, value) {
    this.$set = {
      [property]: value
    };
  }
}

module.exports = SetUpdate;
