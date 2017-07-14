class IncMoneyUpdate {
  constructor(property, change) {
    this.$inc = {
      [property]: Math.round(change * 100)
    };
  }
}

module.exports = IncMoneyUpdate;