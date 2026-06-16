# 0004. Asset Request Specification Workflow

Date: 2026-06-16

## Decision

When the project requires new images, sprites, tilesets, UI elements, audio, fonts, VFX, animations, or other external assets, Codex must provide a detailed asset request specification instead of silently assuming or importing assets.

Codex must not directly import third-party assets, generate final assets, or use random placeholders unless the project owner explicitly approves that task.

## Required Information

Each asset request should include:

* purpose;
* asset type;
* file format;
* resolution or size;
* tile size or atlas layout when relevant;
* pivot, anchor, or origin;
* animation frame count and direction count when relevant;
* style constraints;
* implementation path;
* whether the asset is temporary or production-intended;
* licensing and attribution requirements.

## Rationale

The project is developed by a solo developer and will rely on purchased assets, AI-generated materials, and manually edited resources.

Unspecified asset requirements cause rework, inconsistent style, licensing risks, and broken Unity imports.

A structured asset request workflow keeps Codex in proposal-only mode and allows the project owner to decide how each asset is created, purchased, generated, edited, and imported.

## Status

Accepted.
