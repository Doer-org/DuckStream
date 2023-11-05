/** @type {import('next').NextConfig} */
const nextConfig = {
  images: {
    domains: ["storage.googleapis.com"],
  },
  webpack: (config) => {
    config.externals = [...config.externals, { canvas: "canvas" }]; // required to make Konva & react-konva work
    return config;
  },
};

module.exports = nextConfig;
