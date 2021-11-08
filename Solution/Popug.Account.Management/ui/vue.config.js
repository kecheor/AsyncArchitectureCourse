module.exports = {
  outputDir: "../wwwroot",
  devServer: {
    proxy: {
      "^/api/": {
        target: "https://localhost:7144/",
        pathRewrite: { "^/api/": "/api/" },
        changeOrigin: true,
        logLevel: "debug"
      },
      "^/bff/": {
        target: "https://localhost:7144/",
        pathRewrite: { "^/bff/": "/bff/" },
        changeOrigin: true,
        logLevel: "debug"
      }
    }
  }
};
