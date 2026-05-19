const { defineConfig } = require("cypress");

module.exports = defineConfig({
  e2e: {
    // Your app is redirecting to HTTPS on this port.
    baseUrl: "https://localhost:7228",

    viewportWidth: 1440,
    viewportHeight: 900,
    video: false,
    screenshotOnRunFailure: true,

    setupNodeEvents(on, config) {
      return config;
    },
  },
});
