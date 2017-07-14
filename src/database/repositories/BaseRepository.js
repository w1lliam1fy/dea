const IdQuery = require('../queries/IdQuery.js');

class BaseRepository {
  constructor(collection) {
    this.collection = collection;
  }

  async any(filter) {
    const result = await this.collection.count(filter);

    return result !== 0;
  }

  findMany(filter) {
    return this.collection.find(filter).toArray();
  }

  findOne(filter) {
    return this.collection.findOne(filter);
  }

  findById(id) {
    return this.findOne(new IdQuery(id));
  }

  async insertMany(documents) {
    const result = await this.collection.insertMany(documents);

    return result.ops;
  }

  async insertOne(document) {
    const result = await this.collection.insertOne(document);

    return result.ops[0];
  }

  updateMany(filter, update) {
    return this.collection.updateMany(filter, update);
  }

  updateOne(filter, update, upsert = false) {
    return this.collection.updateOne(filter, update, { upsert: upsert });
  }

  updateById(id, update, upsert = false) {
    return this.updateOne(new IdQuery(id), update, upsert);
  }

  async findOneAndUpdate(filter, update, upsert = false) {
    const result = await this.collection.findOneAndUpdate(filter, update, { upsert: upsert, returnOriginal: false });

    return result.value;
  }

  findByIdAndUpdate(id, update, upsert = false) {
    return this.findOneAndUpdate(new IdQuery(id), update, upsert);
  }

  deleteMany(filter) {
    return this.collection.deleteMany(filter);
  }

  deleteOne(filter) {
    return this.collection.deleteOne(filter);
  }

  deleteById(id) {
    return this.deleteOne(new IdQuery(id));
  }

  async findOneAndDelete(filter) {
    const result = await this.collection.findOneAndDelete(filter);

    return result.value;
  }

  findByIdAndDelete(id) {
    return this.findOneAndDelete(new IdQuery(id));
  }
}

module.exports = BaseRepository;
