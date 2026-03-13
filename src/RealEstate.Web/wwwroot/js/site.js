(() => {
  const header = document.getElementById("siteHeader");

  const updateHeaderState = () => {
    if (!header) {
      return;
    }

    header.classList.toggle("scrolled", window.scrollY > 60);
  };

  const wireWordReveal = () => {
    document.querySelectorAll("[data-word-reveal]").forEach(element => {
      if (element.dataset.enhanced === "true") {
        return;
      }

      const words = (element.textContent || "").trim().split(/\s+/);
      element.textContent = "";

      words.forEach((word, index) => {
        const span = document.createElement("span");
        span.textContent = `${word} `;
        span.style.animationDelay = `${index * 0.08}s`;
        element.appendChild(span);
      });

      element.dataset.enhanced = "true";
    });
  };

  const wireCardParallax = () => {
    document.querySelectorAll(".js-parallax-card").forEach(card => {
      const image = card.querySelector(".js-parallax-image");
      if (!image || card.dataset.parallaxBound === "true") {
        return;
      }

      card.addEventListener("mousemove", event => {
        const bounds = card.getBoundingClientRect();
        const offsetX = ((event.clientX - bounds.left) / bounds.width - 0.5) * 10;
        const offsetY = ((event.clientY - bounds.top) / bounds.height - 0.5) * 10;
        image.style.transform = `scale(1.07) translate(${offsetX}px, ${offsetY}px)`;
      });

      card.addEventListener("mouseleave", () => {
        image.style.transform = "";
      });

      card.dataset.parallaxBound = "true";
    });
  };

  const wireNotificationPanel = () => {
    const root = document.getElementById("notification-panel-root");
    if (!root) {
      return;
    }

    const refreshUrl = root.dataset.refreshUrl;
    if (!refreshUrl) {
      return;
    }

    const closePanel = () => {
      root.classList.remove("is-open");
      const trigger = root.querySelector(".notification-trigger");
      if (trigger) {
        trigger.setAttribute("aria-expanded", "false");
      }
    };

    const openPanel = () => {
      root.classList.add("is-open");
      const trigger = root.querySelector(".notification-trigger");
      if (trigger) {
        trigger.setAttribute("aria-expanded", "true");
      }
    };

    const refreshPanel = async () => {
      const response = await fetch(refreshUrl, { headers: { "X-Requested-With": "XMLHttpRequest" } });
      if (!response.ok) {
        return;
      }

      root.innerHTML = await response.text();
      wirePanel();
      openPanel();
    };

    const wirePanel = () => {
      const trigger = root.querySelector(".notification-trigger");
      if (trigger) {
        trigger.addEventListener("click", event => {
          event.preventDefault();
          event.stopPropagation();
          const isOpen = root.classList.toggle("is-open");
          trigger.setAttribute("aria-expanded", isOpen ? "true" : "false");
        });
      }

      const refreshButton = root.querySelector(".notification-refresh");
      if (refreshButton) {
        refreshButton.addEventListener("click", async event => {
          event.preventDefault();
          await refreshPanel();
        });
      }

      root.querySelectorAll(".notification-form").forEach(form => {
        form.addEventListener("submit", async event => {
          event.preventDefault();

          const response = await fetch(form.action, {
            method: form.method,
            body: new FormData(form),
            headers: { "X-Requested-With": "XMLHttpRequest" }
          });

          if (response.ok) {
            await refreshPanel();
          }
        });
      });
    };

    document.addEventListener("click", event => {
      if (!root.contains(event.target)) {
        closePanel();
      }
    });

    document.addEventListener("keydown", event => {
      if (event.key === "Escape") {
        closePanel();
      }
    });

    wirePanel();
  };

  const wireFooterReveal = () => {
    const footer = document.getElementById("siteFooter");
    if (!footer || typeof IntersectionObserver === "undefined") {
      if (footer) {
        footer.classList.add("is-visible");
      }
      return;
    }

    const observer = new IntersectionObserver(entries => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          footer.classList.add("is-visible");
          observer.disconnect();
        }
      });
    }, { threshold: 0.2 });

    observer.observe(footer);
  };

  updateHeaderState();
  wireWordReveal();
  wireCardParallax();
  wireNotificationPanel();
  wireFooterReveal();

  window.addEventListener("scroll", updateHeaderState, { passive: true });
})();
