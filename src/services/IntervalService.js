class IntervalService {
  initiate(client) {
    require('../intervals/autoUnmute.js')(client);
  }
}

module.exports = new IntervalService();
