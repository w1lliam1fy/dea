class PromiseUtil {
  static async wait(ms) {
    setTimeout(() => {
      return;
    }, ms);
  }
}

module.exports = PromiseUtil;
