class IntervalService {
  initiate(client) {
    require('../intervals/autoUnmute.js')(client);
    require('../intervals/fine.js')(client);
  }
}

module.exports = new IntervalService();
